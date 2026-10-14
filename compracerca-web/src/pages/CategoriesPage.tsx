import React, { useEffect, useState } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { useAuth } from '../context/useAuth';

interface Category {
    id: number;
    name: string;
    description: string;
    isActive: boolean;
}

interface Product {
    id: number;
    title: string;
    description: string;
    price: number;
    city: string;
    categoryName: string;
    isActive: boolean;
}

export default function CategoriesPage() {
    const { id } = useParams<{ id: string }>();
    const [categories, setCategories] = useState<Category[]>([]);
    const [products, setProducts] = useState<Product[]>([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);
    const { user } = useAuth();
    const navigate = useNavigate();

    useEffect(() => {
        const loadData = async () => {
            try {
                setLoading(true);

                // Cargar categorías
                const catResponse = await fetch('/api/categories', {
                    headers: {
                        'Content-Type': 'application/json',
                    },
                });

                if (!catResponse.ok) {
                    throw new Error('Error al cargar categorías');
                }
                const categoriesData = await catResponse.json();
                setCategories(categoriesData);

                // Si hay ID de categoría, filtrar productos; sino mostrar todas
                if (id) {
                    const catId = parseInt(id, 10);
                    const prodResponse = await fetch(`/api/products/paged?pageNumber=1&pageSize=6&categoryId=${catId}`, {
                        headers: {
                            'Content-Type': 'application/json',
                        },
                    });

                    if (!prodResponse.ok) {
                        throw new Error('Error al cargar productos');
                    }
                    const prodData = await prodResponse.json();
                    setProducts(prodData.items);
                } else {
                    // Mostrar todos los productos
                    const prodResponse = await fetch('/api/products/paged?pageNumber=1&pageSize=6', {
                        headers: {
                            'Content-Type': 'application/json',
                        },
                    });

                    if (!prodResponse.ok) {
                        throw new Error('Error al cargar productos');
                    }
                    const prodData = await prodResponse.json();
                    setProducts(prodData.items);
                }

                setLoading(false);
            } catch (err) {
                console.error('Error loading categories:', err);
                setError(
                    err instanceof Error ? err.message : 'Error al cargar categorías y productos.'
                );
                setLoading(false);
            }
        };

        loadData();
    }, [id, navigate]);

    if (loading) {
        return (
            <div style={{
                padding: '2rem',
                textAlign: 'center',
                color: 'white',
                minHeight: '400px',
            }}>
                Cargando categorías...
            </div>
        );
    }

    if (error) {
        return (
            <div style={{
                padding: '2rem',
                backgroundColor: '#ef4444',
                color: 'white',
                borderRadius: '8px',
                margin: '2rem 0',
            }}>
                <p>{error}</p>
                <button
                    onClick={() => window.location.reload()}
                    style={{
                        marginTop: '0.5rem',
                        padding: '0.5rem 1rem',
                        backgroundColor: '#3b82f6',
                        color: 'white',
                        border: 'none',
                        borderRadius: '4px',
                        cursor: 'pointer',
                    }}
                >
                    Reintentar
                </button>
            </div>
        );
    }

    return (
        <div style={{
            maxWidth: '1200px',
            margin: '0 auto',
            padding: '2rem',
        }}>
            <h1 style={{ marginBottom: '1.5rem' }}>
                Categorías{' '}
                {id ? (
                    <span style={{ textTransform: 'capitalize' }}>
                        {id === '1' && 'Electrónica'}{
                        id === '2' && 'Ropa'}{
                        id === '3' && 'Hogar'}{
                        id === '4' && 'Deportes'}{
                        id === '5' && 'Alimentos'}
                    </span>
                ) : 'Todas'}
            </h1>

            {/* Grid de productos filtrados por categoría */}
            <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fill, minmax(280px, 1fr))', gap: '1.5rem' }}>
                {products.map((product) => (
                    <ProductCardCategory key={product.id} product={product} />
                ))}

                {/* Estado vacío */}
                {products.length === 0 && (
                    <div style={{
                        padding: '2rem',
                        textAlign: 'center',
                        color: '#6b7280',
                        gridColumn: '1 / -1',
                    }}>
                        <p>No hay productos en esta categoría.</p>
                    </div>
                )}
            </div>

            <p style={{ marginTop: '2rem', color: '#6b7280', fontSize: '0.875rem' }}>
                {user ? 'Bienvenido de vuelta!' : 'Regístrate o inicia sesión para filtrar por categoría'}
            </p>
        </div>
    );
}

// Componente tarjeta de producto para categoría
const ProductCardCategory = ({ product }: { product: Product }) => {
    return (
        <div style={{
            border: '1px solid #e5e7eb',
            borderRadius: '8px',
            padding: '1rem',
            textAlign: 'center',
            transition: 'transform 0.2s',
            marginBottom: '1rem',
            backgroundColor: '#ffffff',
        }}>
            <h3 style={{ fontSize: '0.875rem', color: '#374151', marginBottom: '0.5rem' }}>
                {product.title}
            </h3>
            <p style={{ fontSize: '0.75rem', color: '#6b7280', marginBottom: '0.5rem', height: '3em', overflow: 'hidden' }}>
                {product.description}
            </p>
            <div style={{ fontSize: '1.25rem', fontWeight: 'bold', color: '#1f2937', margin: '0.5rem 0' }}>
                COP {product.price.toLocaleString()}
            </div>
            <div style={{ fontSize: '0.75rem', color: '#6b7280' }}>
                por {product.city}
            </div>
            <a
                href={`/productos/${product.id}`}
                style={{
                    display: 'inline-block',
                    marginTop: '0.5rem',
                    padding: '0.5rem 1rem',
                    backgroundColor: '#3b82f6',
                    color: 'white',
                    textDecoration: 'none',
                    borderRadius: '4px',
                    fontSize: '0.75rem',
                }}
            >
                Ver detalles
            </a>
        </div>
    );
};