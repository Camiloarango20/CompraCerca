import { createContext } from 'react';

export interface User {
    email: string;
    role: string;
}

export interface AuthContextType {
    user: User | null;
    token: string | null;
    isAuthenticated: boolean;
    login: (credentials: import('../interfaces/auth').LoginRequest) => Promise<void>;
    register: (data: import('../interfaces/auth').RegisterRequest) => Promise<void>;
    logout: () => void;
    loading: boolean;
}

export const AuthContext = createContext<AuthContextType | undefined>(undefined);
