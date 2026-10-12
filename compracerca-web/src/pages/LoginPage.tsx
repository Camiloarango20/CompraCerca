import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../context/useAuth';
import { AxiosError } from 'axios';

export function LoginPage() {
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const [firstName, setFirstName] = useState('');
    const [lastName, setLastName] = useState('');
    const [city, setCity] = useState('');
    const [error, setError] = useState<string | null>(null);
    const [isSubmitting, setIsSubmitting] = useState(false);
    const [showRegister, setShowRegister] = useState(false);

    const { login, register } = useAuth();
    const navigate = useNavigate();

    const handleLoginSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        setError(null);
        setIsSubmitting(true);

        try {
            await login({ email, password });
            navigate('/');
        } catch (err) {
            if (err instanceof AxiosError && err.response?.data?.message) {
                setError(err.response.data.message);
            } else {
                setError('Credenciales inválidas o error de conexión con la API.');
            }
        } finally {
            setIsSubmitting(false);
        }
    };

    const handleRegisterSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        setError(null);
        setIsSubmitting(true);

        try {
            const data = {
                firstName: firstName.trim(),
                lastName: lastName.trim(),
                email: email.trim().toLowerCase(),
                password: password,
                city: city.trim()
            };
            await register(data);
            setShowRegister(false);
            setIsSubmitting(false);
            navigate('/');
        } catch (err) {
            if (err instanceof AxiosError && err.response?.data?.message) {
                setError(err.response.data.message);
            } else {
                setError('Error al registrarse. Verifique que la contraseña tenga al menos 12 caracteres y todos los campos estén completos.');
            }
        } finally {
            setIsSubmitting(false);
        }
    };

    return (
        <div style={{
            maxWidth: '400px',
            margin: '4rem auto',
            padding: '2rem',
            border: '1px solid #ccc',
            borderRadius: '8px',
            boxShadow: '0 2px 8px rgba(0,0,0,0.1)',
        }}>
            <h2>{ showRegister ? 'Registro - CompraCerca' : 'Iniciar Sesión - CompraCerca' }</h2>

            { error && (
                <div style={{
                    backgroundColor: '#ffebee',
                    color: '#c62828',
                    padding: '0.75rem',
                    borderRadius: '4px',
                    marginBottom: '1rem',
                }}>
                    { error }
                </div>
            )}

            <form onSubmit={ showRegister ? handleRegisterSubmit : handleLoginSubmit }>
                { showRegister ? (
                    <>
                        <div style={{ marginBottom: '1rem' }}>
                            <label style={{ display: 'block', marginBottom: '0.5rem' }}>Nombre</label>
                            <input
                                type="text"
                                value={ firstName }
                                onChange={(e) => setFirstName(e.target.value)}
                                required
                                style={{ width: '100%', padding: '0.5rem', boxSizing: 'border-box' }}
                            />
                        </div>

                        <div style={{ marginBottom: '1.5rem' }}>
                            <label style={{ display: 'block', marginBottom: '0.5rem' }}>Apellido</label>
                            <input
                                type="text"
                                value={ lastName }
                                onChange={(e) => setLastName(e.target.value)}
                                required
                                style={{ width: '100%', padding: '0.5rem', boxSizing: 'border-box' }}
                            />
                        </div>

                        <div style={{ marginBottom: '1.5rem' }}>
                            <label style={{ display: 'block', marginBottom: '0.5rem' }}>Correo Electrónico</label>
                            <input
                                type="email"
                                value={ email }
                                onChange={(e) => setEmail(e.target.value)}
                                required
                                style={{ width: '100%', padding: '0.5rem', boxSizing: 'border-box' }}
                            />
                        </div>

                        <div style={{ marginBottom: '1.5rem' }}>
                            <label style={{ display: 'block', marginBottom: '0.5rem' }}>Contraseña</label>
                            <input
                                type="password"
                                value={ password }
                                onChange={(e) => setPassword(e.target.value)}
                                required
                                minLength={12}
                                style={{ width: '100%', padding: '0.5rem', boxSizing: 'border-box' }}
                            />
                            <small style={{ color: '#666', fontSize: '0.75rem' }}>
                                La contraseña debe tener al menos 12 caracteres
                            </small>
                        </div>

                        <div style={{ marginBottom: '1.5rem' }}>
                            <label style={{ display: 'block', marginBottom: '0.5rem' }}>Ciudad</label>
                            <input
                                type="text"
                                value={ city }
                                onChange={(e) => setCity(e.target.value)}
                                required
                                style={{ width: '100%', padding: '0.5rem', boxSizing: 'border-box' }}
                            />
                        </div>
                    </>
                ) : (
                    <>
                        <div style={{ marginBottom: '1rem' }}>
                            <label style={{ display: 'block', marginBottom: '0.5rem' }}>Correo Electrónico</label>
                            <input
                                type="email"
                                value={ email }
                                onChange={(e) => setEmail(e.target.value)}
                                required
                                style={{ width: '100%', padding: '0.5rem', boxSizing: 'border-box' }}
                            />
                        </div>

                        <div style={{ marginBottom: '1.5rem' }}>
                            <label style={{ display: 'block', marginBottom: '0.5rem' }}>Contraseña</label>
                            <input
                                type="password"
                                value={ password }
                                onChange={(e) => setPassword(e.target.value)}
                                required
                                style={{ width: '100%', padding: '0.5rem', boxSizing: 'border-box' }}
                            />
                        </div>
                    </>
                )}

                <button
                    type="submit"
                    disabled={ isSubmitting }
                    style={{
                        width: '100%',
                        padding: '0.75rem',
                        backgroundColor: '#1976d2',
                        color: '#fff',
                        border: 'none',
                        borderRadius: '4px',
                        cursor: 'pointer',
                    }}
                >
                    { isSubmitting ? (showRegister ? 'Registrando...' : 'Ingresando...') : (showRegister ? 'Registrarse' : 'Iniciar Sesión') }
                </button>
            </form>
        </div>
    );
}