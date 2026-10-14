import axios from 'axios';

const API_URL = import.meta.env.VITE_API_URL || 'https://localhost:7115/api';

const axiosClient = axios.create({
    baseURL: API_URL,
    headers: {
        'Content-Type': 'application/json',
    },
});

axiosClient.interceptors.request.use(
    (config) => {
        const token = localStorage.getItem('token');
        if (token && config.headers) {
            config.headers.Authorization = `Bearer ${token}`;
        }
        return config;
    },
    (error) => Promise.reject(error)
);

axiosClient.interceptors.response.use(
    (response) => response,
    (error) => {
        if (error.response) {
            // El servidor respondió con un código de estado fuera del rango 2xx
            const statusCode = error.response.status;
            const errorMessage = error.response.data?.message || error.response.data?.detail || 'Error en la solicitud';

            // Mensajes personalizados por código de estado
            const statusMessages: Record<number, string> = {
                400: 'Solicitud incorrecta: verifique los datos enviados',
                401: 'No autorizado: sesión expirada o credenciales inválidas',
                403: 'Prohibido: no tiene permisos para esta operación',
                404: 'No encontrado: el recurso solicitado no existe',
                500: 'Error interno del servidor: contacte al administrador',
                502: 'Bad Gateway: la API no está respondiendo correctamente',
                503: 'Servicio no disponible: la API está temporalmente fuera de línea',
            };

            // Almacenar información del error para mostrar en la UI
            if (statusCode === 401) {
                localStorage.removeItem('token');
                if (window.location.pathname !== '/login') {
                    window.location.href = '/login';
                }
            }

            // Rechazar con información enriquecida
            error.response.statusMessageCustom = statusMessages[statusCode] || errorMessage;
        }

        return Promise.reject(error);
    }
);

export default axiosClient;