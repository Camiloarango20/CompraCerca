import React from 'react';
import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import { AuthProvider } from './context/AuthContext';
import { ProtectedRoute } from './components/ProtectedRoute';
import { LoginPage } from './pages/LoginPage';

function CatalogPage() {
    return (
        <div style= {{ padding: '2rem' }
}>
    <h1>Catálogo de Productos </h1>
        </div>
  );
}

function AdminPage() {
    return (
        <div style= {{ padding: '2rem' }
}>
    <h1>Panel de Administración </h1>
        </div>
  );
}

export default function App() {
    return (
        <AuthProvider>
        <BrowserRouter>
        <Routes>
        {/* Rutas Públicas */ }
        < Route path = "/" element = {< CatalogPage />} />
            < Route path = "/login" element = {< LoginPage />} />

{/* Rutas Protegidas */ }
<Route element={ <ProtectedRoute requiredRole="Admin" />}>
    <Route path="/admin" element = {< AdminPage />} />
        </Route>

{/* Redirección por defecto */ }
<Route path="*" element = {< Navigate to = "/" replace />} />
    </Routes>
    </BrowserRouter>
    </AuthProvider>
  );
}