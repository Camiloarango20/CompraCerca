import React, { useEffect, useState } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { useAuth } from '../context/useAuth';

interface Product {
    id: number;
    title: string;
    description: string;
    price: number;
    city: string;
    categoryName: string;
    isActive: boolean;
    userId: number;
}

export default function ProductDetailPage() {
    const { id } = useParams<{ id: string }>();
    const [product, setProduct] = useState<Product | null>(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);
    const { user, logout } = useAuth();
    const navigate = useNavigate();

    useEffect(() => {
        const fetchProduct = async () => {
            try {
                setLoading(true);
                const response = await fetch(`/api/products/${id}`, {
                    headers: {
                        'Content-Type': 'application/json',
                    },
                });

                if (!response.ok) {
                    const errorData = await response.json();
                    throw new Error(errorData.message || 'Producto no encontrado');
                }

                const data = await response.json();
                setProduct(data);
                setLoading(false);
            } catch (err) {
                console.error('Error fetching product:', err);
                setError(
                    err instanceof Error ? err.message : 'Error al cargar el producto.'
                );
                setLoading(false);
                navigate('/');
            }
        };

        if (id) {
            fetchProduct();
        }
    }, [id, navigate]);

    if (loading) {
        return (
            <div style={{
                padding: '2rem',
                textAlign: 'center',
                color: 'white',
                minHeight: '400px',
            }}>
                Cargando detalle del producto...
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

    if (!product) {
        return <div>Producto no disponible</div>;
    }

    return (
        <>
            <div style={{
                maxWidth: '800px',
                margin: '0 auto',
                padding: '2rem',
            }}>
                <h1 style={{ marginBottom: '1rem' }}>
                    {product.title}
                </h1>

                <div style={{
                    backgroundColor: '#f3f4f6',
                    borderRadius: '8px',
                    padding: '1rem',
                    marginBottom: '1.5rem',
                }}>
                    <p style={{ fontSize: '0.875rem', color: '#6b7280', marginBottom: '0.5rem' }}>
                        Descripción:
                    </p>
                    <p style={{ fontSize: '1rem', lineHeight: '1.6' }}>
                        {product.description}
                    </p>
                </div>

                <div style={{ marginBottom: '1.5rem' }}>
                    <span style={{ fontSize: '1.5rem', fontWeight: 'bold', color: '#1f2937' }}>
                        COP {product.price.toLocaleString()}
                    </span>
                    <span style={{ fontSize: '0.875rem', color: '#6b7280', marginLeft: '1rem' }}>
                        por {product.city}
                    </span>
                </div>

                <div style={{
                    backgroundColor: '#f3f4f6',
                    borderRadius: '8px',
                    padding: '1rem',
                    marginBottom: '1.5rem',
                }}>
                    <p style={{ fontSize: '0.875rem', color: '#374151', marginBottom: '0.5rem' }}>
                        Categoría:
                    </p>
                    <p style={{ fontSize: '1rem', color: '#1f2937' }}>
                        {product.categoryName}
                    </p>
                </div>

                {/* Botones de acción dependiendo de si el usuario es el dueño o admin */}
                {user && (product.userId === 1 || user.role === 'Admin') && (
                    <div style={{
                        marginTop: '1.5rem',
                        padding: '1rem',
                        backgroundColor: '#f3f4f6',
                        borderRadius: '8px',
                    }}>
                        <p style={{ fontSize: '0.875rem', color: '#374151' }}>
                            {user.role === 'Admin'
                                ? 'Producto gestionado por un administrador'
                                : 'Soy el dueño de este producto'}
                        </p>
                    </div>
                )}

                {/* Botón de agregar al carrito (siempre visible para usuarios autenticados) */}
                {user && (
                    <div style={{
                        marginTop: '1.5rem',
                        padding: '1rem',
                        backgroundColor: '#e0f7fa',
                        borderRadius: '8px',
                        border: '2px solid #0284c7',
                    }}>
                        <p style={{ fontSize: '0.875rem', color: '#0891b2', marginBottom: '0.5rem' }}>
                            Agregar al carrito
                        </p>
                        <button
                            onClick={() => alert('Producto agregado al carrito (funcionalidad simulada)')}
                            style={{
                                width: '100%',
                                padding: '0.75rem',
                                backgroundColor: '#0284c7',
                                color: 'white',
                                border: 'none',
                                borderRadius: '4px',
                                fontSize: '1rem',
                                cursor: 'pointer',
                                marginTop: '0.5rem',
                            }}
                        >
                            Agregar al carrito
                        </button>
                    </div>
                </div>

                <p style={{ marginTop: '2rem', color: '#6b7280', fontSize: '0.875rem' }}>
                    {user ? 'Usuario autenticado' : 'Inicia sesión para interactuar con el producto'}
                </p>
            </div>
        </>
    );
}