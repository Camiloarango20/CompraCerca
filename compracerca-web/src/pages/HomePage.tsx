import React, { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../context/useAuth';
import { productService } from '../services/api/axiosClient'; // Usaremos axiosClient directamente

interface Product {
    id: number;
    title: string;
    description: string;
    price: number;
    city: string;
    categoryName: string;
    isActive: boolean;
}

// Componente tarjeta de producto
const ProductCard = ({ product }: { product: Product }) => {
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

export default function HomePage() {
    const [products, setProducts] = useState<Product[]>([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);
    const { user } = useAuth();
    const navigate = useNavigate();

    useEffect(() => {
        const fetchProducts = async () => {
            try {
                setLoading(true);
                // Llamada a la API paginada - traemos la primera página
                const response = await fetch('/api/products/paged?pageNumber=1&pageSize=6', {
                    headers: {
                        'Content-Type': 'application/json',
                    },
                });

                if (!response.ok) {
                    const errorData = await response.json();
                    throw new Error(errorData.message || 'Error al cargar productos');
                }

                const data = await response.json();
                setProducts(data.items);
                setLoading(false);
            } catch (err) {
                console.error('Error fetching products:', err);
                setError(
                    err instanceof Error ? err.message : 'Error al cargar productos. Por favor, inténtelo de nuevo.'
                );
                setLoading(false);
            }
        };

        fetchProducts();
    }, [navigate]);

    if (loading) {
        return (
            <div style={{
                padding: '2rem',
                textAlign: 'center',
                color: 'white',
                minHeight: '400px',
            }}>
                Cargando productos...
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
                ¡Bienvenido a CompraCerca!
                {user ? `, ${user.email.split('@')[0]}` : ''}
            </h1>

            {/* Hero/Banner section */}
            <div style={{
                background: 'linear-gradient(135deg, #3b82f6, #1d4ed8)',
                color: 'white',
                padding: '2rem',
                borderRadius: '12px',
                marginBottom: '2rem',
                textAlign: 'center',
            }}>
                <h2 style={{ fontSize: '2rem', marginBottom: '0.5rem' }}>Encuentra lo que necesitas</h2>
                <p style={{ fontSize: '1rem', opacity: 0.9 }}>
                    Marketplace para encontrar productos nuevos y usados en Colombia
                </p>
            </div>

            {/* Grid de productos */}
            <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fill, minmax(280px, 1fr))', gap: '1.5rem' }}>
                {products.map((product) => (
                    <ProductCard key={product.id} product={product} />
                ))}

                {/* Estado vacío */}
                {products.length === 0 && (
                    <div style={{
                        padding: '2rem',
                        textAlign: 'center',
                        color: '#6b7280',
                        gridColumn: '1 / -1',
                    }}>
                        <p>No hay productos disponibles en este momento.</p>
                    </div>
                )}
            </div>

            <p style={{ marginTop: '2rem', color: '#6b7280', fontSize: '0.875rem' }}>
                {user ? 'Bienvenido de vuelta!' : 'Regístrate o inicia sesión para publicar productos'}
            </p>
        </div>
    );
}