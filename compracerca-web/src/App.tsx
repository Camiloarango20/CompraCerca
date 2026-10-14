import React from 'react';
import { BrowserRouter, Routes, Route, Link } from 'react-router-dom';
import { useAuth } from './src/context/useAuth';
import { AxiosError } from 'axios';

import HomePage from './src/pages/HomePage';
import CategoriesPage from './src/pages/CategoriesPage';
import ProductDetailPage from './src/pages/ProductDetailPage';
import CartPage from './src/pages/CartPage';
import RegisterPage from './src/pages/RegisterPage';
import LoginPage from './src/pages/LoginPage';
import ProtectedRoute from './src/components/ProtectedRoute';
import NotFoundPage from './src/pages/NotFoundPage';

const NavBar = () => {
    const { isAuthenticated, user, logout } = useAuth();

    return (
        <nav style={{
            backgroundColor: '#1f2937',
            padding: '1rem',
            color: 'white',
            minHeight: '64px',
            display: 'flex',
            alignItems: 'center',
            justifyContent: 'space-between',
            marginBottom: '2rem',
        }}>
            <div style={{ display: 'flex', alignItems: 'center', gap: '1rem' }}>
                <Link to="/" style={{ fontSize: '1.5rem', fontWeight: 'bold', color: 'white', textDecoration: 'none' }}>
                    CompraCerca
                </Link>
                <input
                    type="text"
                    placeholder="Buscar..."
                    style={{
                        padding: '0.5rem 1rem',
                        borderRadius: '4px',
                        border: 'none',
                        backgroundColor: '#3f87a1',
                        color: 'white',
                        width: '200px',
                    }}
                />
            </div>

            <div style={{ display: 'flex', alignItems: 'center', gap: '1rem' }}>
                {isAuthenticated ? (
                    <>
                        <span style={{ fontWeight: 'bold' }}>Hola, {user?.email?.split('@')[0] || 'Usuario'}</span>
                        <button
                            onClick={logout}
                            style={{
                                background: 'none',
                                border: 'none',
                                color: '#e2e8f0',
                                fontSize: '0.875rem',
                                cursor: 'pointer',
                            }}
                        >
                            Cerrar sesión
                        </button>
                    </>
                ) : (
                    <>
                        <Link to="/login" style={{ color: '#e2e8f0', textDecoration: 'none' }}>Iniciar</Link>
                        <Link to="/registro" style={{ color: '#e2e8f0', textDecoration: 'none' }}>Registro</Link>
                    </>
                )}
            </div>
        </nav>
    );
};

export default function App() {
    const { isAuthenticated, loading } = useAuth();

    if (loading) {
        return (
            <div style={{
                minHeight: '100vh',
                display: 'flex',
                alignItems: 'center',
                justifyContent: 'center',
                backgroundColor: '#1f2937',
                color: 'white',
            }}>
                Cargando sesión...
            </div>
        );
    }

    return (
        <BrowserRouter>
            <NavBar />

            <Routes>
                <Route path="/" element={<HomePage />} />
                <Route path="/categorias/:id" element={<CategoriesPage />} />
                <Route path="/productos/:id" element={<ProductDetailPage />} />
                <Route path="/carrito" element={<CartPage />} />
                <Route path="/registro" element={<RegisterPage />} />
                <Route path="/login" element={<LoginPage />} />
                <Route path="/perfil" element={<ProtectedRoute requiredRole="Admin" />} />
                <Route path="*" element={<NotFoundPage />} />
            </Routes>
        </BrowserRouter>
    );
}