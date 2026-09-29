import React from 'react';
import { Navigate, Outlet } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';

interface ProtectedRouteProps {
    requiredRole?: string;
}

export const ProtectedRoute: React.FC<ProtectedRouteProps> = ({ requiredRole }) => {
    const { isAuthenticated, user, loading } = useAuth();

    if (loading) {
        return (
            <div style= {{ padding: '2rem', textAlign: 'center' }
    }>
        Cargando...
    </div>
    );
  }

if (!isAuthenticated) {
    return <Navigate to="/login" replace />;
}

if (requiredRole && user?.role !== requiredRole) {
    return <Navigate to="/" replace />;
}

return <Outlet />;
};