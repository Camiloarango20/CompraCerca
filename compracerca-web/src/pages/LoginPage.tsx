import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';
import { AxiosError } from 'axios';

export function LoginPage() {
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const [error, setError] = useState<string | null>(null);
    const [isSubmitting, setIsSubmitting] = useState(false);

    const { login } = useAuth();
    const navigate = useNavigate();

    const handleSubmit = async (e: React.FormEvent) => {
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

    return (
        <div
      style= {{
        maxWidth: '400px',
            margin: '4rem auto',
                padding: '2rem',
                    border: '1px solid #ccc',
                        borderRadius: '8px',
                            boxShadow: '0 2px 8px rgba(0,0,0,0.1)',
      }
}
    >
    <h2>Iniciar Sesión - CompraCerca </h2>

{
    error && (
        <div
          style={
        {
            backgroundColor: '#ffebee',
                color: '#c62828',
                    padding: '0.75rem',
                        borderRadius: '4px',
                            marginBottom: '1rem',
          }
    }
        >
    { error }
        </div>
      )
}

<form onSubmit={ handleSubmit }>
    <div style={ { marginBottom: '1rem' } }>
        <label style={ { display: 'block', marginBottom: '0.5rem' } }>
            Correo Electrónico
                </label>
                < input
type = "email"
value = { email }
onChange = {(e) => setEmail(e.target.value)}
required
style = {{ width: '100%', padding: '0.5rem', boxSizing: 'border-box' }}
          />
    </div>

    < div style = {{ marginBottom: '1.5rem' }}>
        <label style={ { display: 'block', marginBottom: '0.5rem' } }>
            Contraseña
            </label>
            < input
type = "password"
value = { password }
onChange = {(e) => setPassword(e.target.value)}
required
style = {{ width: '100%', padding: '0.5rem', boxSizing: 'border-box' }}
          />
    </div>

    < button
type = "submit"
disabled = { isSubmitting }
style = {{
    width: '100%',
        padding: '0.75rem',
            backgroundColor: '#1976d2',
                color: '#fff',
                    border: 'none',
                        borderRadius: '4px',
                            cursor: 'pointer',
          }}
        >
{ isSubmitting? 'Ingresando...': 'Iniciar Sesión' }
    </button>
    </form>
    </div>
  );
}