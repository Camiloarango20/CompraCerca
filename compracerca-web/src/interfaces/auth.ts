export interface LoginRequest {
    email: string;
    password: string;
}

export interface RegisterRequest {
    firstName: string;
    lastName: string;
    email: string;
    password: string;
    city: string;
    role?: string;
}

export interface AuthResponse {
    token: string;
    email: string;
    role: string;
    expiration: string;
}

export interface UserTokenPayload {
    nameid?: string;
    email: string;
    role: string;
    exp: number;
    'http://schemas.microsoft.com/ws/2008/06/identity/claims/role'?: string;
}