export interface LoginRequest {
  usernameOrEmail: string;
  password: string;
}

export interface LoginResponse {
  token: string;
  username: string;
  email: string;
  expiresAt: string;
}

export interface RegisterRequest {
  username: string;
  email: string;
  password: string;
  otp?: string;
}

export interface AuthResponse {
  message?: string;
  token?: string;
  username?: string;
  email?: string;
}

export interface ErrorResponse {
  error?: {
    message?: string;
  };
}