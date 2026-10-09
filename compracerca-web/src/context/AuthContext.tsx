import React, { createContext, useContext, useState, ReactNode, useEffect } from 'react';
import { jwtDecode } from 'jwt-decode';
import { authService } from '../services/authService';
import { LoginRequest, RegisterRequest, UserTokenPayload } from '../interfaces/auth';

interface User {
    email: string;
    role: string;
}

interface AuthContextType {
    user: User | null;
    token: string | null;
    isAuthenticated: boolean;
    login: (credentials: LoginRequest) => Promise<void>;
    register: (data: RegisterRequest) => Promise<void>;
    logout: () => void;
    loading: boolean;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

const getUserFromStoredToken = (): { user: User | null; token: string | null } => {
    const storedToken = localStorage.getItem('token');
    if (!storedToken) return { user: null, token: null };

    try {
        const decoded = jwtDecode<UserTokenPayload>(storedToken);

        if (decoded.exp * 1000 < Date.now()) {
            localStorage.removeItem('token');
            return { user: null, token: null };
        }

        const roleClaim =
            decoded['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] ||
            decoded.role ||
            'User';

        return {
            token: storedToken,
            user: { email: decoded.email, role: roleClaim },
        };
    } catch {
        localStorage.removeItem('token');
        return { user: null, token: null };
    }
};

export function AuthProvider({ children }: { children: ReactNode }) {
    const [authState, setAuthState] = useState<{ user: User | null; token: string | null }>({
        user: null,
        token: null,
    });

    const [loading, setLoading] = useState<boolean>(true);

    useEffect(() => {
        const stored = getUserFromStoredToken();
        setAuthState(stored);
        setLoading(false);
    }, []);

    const logout = () => {
        localStorage.removeItem('token');
        setAuthState({ user: null, token: null });
    };

    const handleAuthResponse = (authToken: string) => {
        localStorage.setItem('token', authToken);

        try {
            const decoded = jwtDecode<UserTokenPayload>(authToken);

            const roleClaim =
                decoded['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] ||
                decoded.role ||
                'User';

            setAuthState({
                token: authToken,
                user: { email: decoded.email, role: roleClaim },
            });
        } catch {
            logout();
        }
    };

    const login = async (credentials: LoginRequest) => {
        setLoading(true);
        const response = await authService.login(credentials);
        handleAuthResponse(response.token);
        setLoading(false);
    };

    const register = async (data: RegisterRequest) => {
        setLoading(true);
        const response = await authService.register(data);
        handleAuthResponse(response.token);
        setLoading(false);
    };

    return (
        <AuthContext.Provider
            value= {{
        user: authState.user,
            token: authState.token,
                isAuthenticated: !!authState.user,
                    login,
                    register,
                    logout,
                    loading,
            }
}
        >
{ children }
    </AuthContext.Provider>
    );
}

export const useAuth = () => {
    const context = useContext(AuthContext);
    if (!context) {
        throw new Error('useAuth debe usarse dentro de un AuthProvider');
    }
    return context;
};
