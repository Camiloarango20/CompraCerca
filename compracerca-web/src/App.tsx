import React, { useState, useEffect } from 'react';
import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';
import AuthProvider from './context/AuthContext.tsx';
import { ProtectedRoute } from './components/ProtectedRoute';
import { LoginPage } from './pages/LoginPage';

// --- Catálogo dinámico (igual que la versión reciente) ---
interface Product {
    id: number;
    title: string;
    description: string;
    price: number;
    categoryName: string;
    userName: string;
    city: string;
    isActive: boolean;
}

interface CatalogState {
    products: Product[];
    loading: boolean;
    error: string | null;
}

async function loadProductsAPI(): Promise<Product[]> {
    try {
        const API_URL = import.meta.env.VITE_API_URL || 'https://localhost:7115/api';
        const response = await fetch(`${API_URL}/api/products`, {
            headers: {
                'Content-Type': 'application/json',
            },
        });
        if (!response.ok) throw new Error('Network response was not ok');
        const json = await response.json();
        return json.filter((p: Product) => p.isActive);
    } catch (err) {
        const errorMessage = err instanceof Error ? err.message : 'Error al cargar productos';
        throw new Error(errorMessage, { cause: err });
    }
}

function CatalogPage() {
    const [state, setState] = useState<CatalogState>({
        products: [],
        loading: true,
        error: null,
    });

    useEffect(() => {
        loadProductsAPI().then(products => {
            setState({ products, loading: false, error: null });
        }).catch(err => {
            setState({ loading: false, error: err.message, products: [] });
        });
    }, []);

    if (state.error) {
        return (
            <div style={{ padding: '2rem', color: '#dc3545', textAlign: 'center' }}>
                <p>Error al cargar productos: {state.error}</p>
                <button
                    onClick={() => {
                        setState(prev => ({ ...prev, loading: true, error: null }));
                        loadProductsAPI().then(products => {
                            setState({ products, loading: false, error: null });
                        });
                    }}
                    style={{
                        marginTop: '1rem',
                        padding: '0.5rem 1rem',
                        backgroundColor: '#198754',
                        color: '#fff',
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

    if (state.loading) {
        return (
            <div style={{ padding: '2rem', textAlign: 'center' }}>
                <div style={{ border: '2px solid #ccc', borderRadius: '8px', padding: '2rem', margin: '2rem auto' }}>
                    <span style={{ fontSize: '2rem', marginRight: '0.5rem' }}>⏳</span>
                    <span>Cargando productos...</span>
                </div>
            </div>
        );
    }

    if (state.products.length === 0) {
        return (
            <div style={{ padding: '2rem', textAlign: 'center', color: '#6c757d' }}>
                <p>No hay productos disponibles en este momento.</p>
                <p style={{ fontSize: '0.9rem', marginTop: '1rem' }}>Volveré pronto.</p>
            </div>
        );
    }

    return (
        <div style={{ padding: '2rem' }}>
            <h1>Catálogo de Productos </h1>
            <div style={{
                display: 'grid',
                gridTemplateColumns: 'repeat(auto-fill, minmax(250px, 1fr))',
                gap: '1.5rem',
                maxWidth: '1200px',
                margin: '0 auto'
            }}>
                {state.products.map((product) => (
                    <div
                        key={product.id}
                        style={{
                            border: '1px solid #e0e0e0',
                            borderRadius: '8px',
                            padding: '1rem',
                            textAlign: 'center',
                            background: '#fff',
                            boxShadow: '0 2px 4px rgba(0,0,0,0.05)',
                            transition: 'transform 0.2s',
                        }}
                    >
                        <div style={{
                            width: '100%', height: '200px', borderRadius: '8px 8px 0 0',
                            backgroundColor: '#e9ecef', overflow: 'hidden', position: 'relative'
                        }}>
                            <span style={{
                                position: 'absolute', top: 0, left: 0, right: 0, bottom: 0,
                                display: 'flex', alignItems: 'center', justifyContent: 'center',
                                color: '#6c757d', fontSize: '2rem'
                            }}>
                                📦
                            </span>
                        </div>

                        <div style={{ padding: '0.75rem' }}>
                            <h4 style={{ fontSize: '1rem', marginBottom: '0.25rem' }}>{product.title}</h4>
                            <p style={{ fontSize: '0.85rem', color: '#666', marginBottom: '0.25rem' }}>
                                {product.categoryName || 'Sin categoría'}
                            </p>
                            <p style={{ fontSize: '1.2rem', margin: '0.1rem 0', color: '#28a745' }}>
                                ${product.price.toFixed(2).replace('.', ',')}
                            </p>
                            <p style={{ fontSize: '0.8rem', color: '#666' }}>
                                Vendedor: {product.userName || 'Anónimo'}
                            </p>
                            <p style={{ fontSize: '0.75rem', color: '#666', marginTop: '0.25rem' }}>
                                {product.city || 'Ciudad no especificada'}
                            </p>
                        </div>
                    </div>
                ))}
            </div>
        </div>
    );
}

// --- Componente App principal ---
export default function App() {
    return (
        <AuthProvider>
        <Router>
        <Routes>
            {/* Rutas Públicicas */}
            <Route path="/" element={<CatalogPage />} />
            <Route path="/login" element={<LoginPage />} />

            {/* Rutas Protegidas - Admin solo */}
            <Route path="/admin" element={<ProtectedRoute requiredRole="Admin" />} />

            {/* Redirección por defecto */}
            <Route path="*" element={<Navigate to="/" replace />} />
        </Routes>
        </Router>
        </AuthProvider>
    );
}