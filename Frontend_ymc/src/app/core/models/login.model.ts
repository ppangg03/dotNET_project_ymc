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