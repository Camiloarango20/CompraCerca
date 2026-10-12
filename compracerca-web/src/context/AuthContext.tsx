
import { useState, type ReactNode } from 'react';
import { jwtDecode } from 'jwt-decode';
import { authService } from '../services/authService';
import type {
    LoginRequest,
    RegisterRequest,
    UserTokenPayload,
} from '../interfaces/auth';
import { AuthContext, type User } from './authContext';

const getUserFromStoredToken = (): {
    user: User | null;
    token: string | null;
} => {
    const storedToken = localStorage.getItem('token');

    if (!storedToken) {
        return { user: null, token: null };
    }

    try {
        const decoded = jwtDecode<UserTokenPayload>(storedToken);

        if (!decoded.exp || decoded.exp * 1000 <= Date.now()) {
            localStorage.removeItem('token');
            return { user: null, token: null };
        }

        const roleClaim =
            decoded[
                'http://schemas.microsoft.com/ws/2008/06/identity/claims/role'
            ] ||
            decoded.role ||
            'User';

        return {
            token: storedToken,
            user: {
                email: decoded.email,
                role: roleClaim,
            },
        };
    } catch {
        localStorage.removeItem('token');
        return { user: null, token: null };
    }
};

const handleAuthResponse = (
    setAuthState: React.Dispatch<React.SetStateAction<{ user: User | null; token: string | null }>>,
    logout: () => void
) => {
    return (authToken: string) => {
        localStorage.setItem('token', authToken);

        try {
            const decoded = jwtDecode<UserTokenPayload>(authToken);

            const roleClaim =
                decoded[
                    'http://schemas.microsoft.com/ws/2008/06/identity/claims/role'
                ] ||
                decoded.role ||
                'User';

            setAuthState({
                token: authToken,
                user: {
                    email: decoded.email,
                    role: roleClaim,
                },
            });
        } catch {
            logout();
        }
    };
};

export default function AuthProvider({ children }: { children: ReactNode }) {
    const [authState, setAuthState] = useState(getUserFromStoredToken);
    const [loading, setLoading] = useState(false);

    const logout = () => {
        localStorage.removeItem('token');
        setAuthState({ user: null, token: null });
    };

    const authResponseHandler = handleAuthResponse(setAuthState, logout);

    const login = async (credentials: LoginRequest) => {
        setLoading(true);

        try {
            const response = await authService.login(credentials);
            authResponseHandler(response.token);
        } finally {
            setLoading(false);
        }
    };

    const register = async (data: RegisterRequest) => {
        setLoading(true);

        try {
            const response = await authService.register(data);
            authResponseHandler(response.token);
        } finally {
            setLoading(false);
        }
    };

    return (
        <AuthContext.Provider
            value={{
                user: authState.user,
                token: authState.token,
                isAuthenticated: !!authState.user,
                login,
                register,
                logout,
                loading,
            }}
        >
            {children}
        </AuthContext.Provider>
    );
}
