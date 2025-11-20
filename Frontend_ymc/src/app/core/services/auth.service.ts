import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, BehaviorSubject, throwError } from 'rxjs';
import { tap, catchError } from 'rxjs/operators';
import { Router } from '@angular/router';
import { environment } from '../../../environments/environment';
import { LoginRequest, LoginResponse } from '../models/login.model';
import { User } from '../models/user.model';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private apiUrl = `${environment.apiUrl}`;
  private tokenKey = 'jwt_token';
  private userKey = 'current_user';
  
  // BehaviorSubject to track authentication state
  private isAuthenticatedSubject = new BehaviorSubject<boolean>(this.hasToken());
  public isAuthenticated$ = this.isAuthenticatedSubject.asObservable();
  
  private currentUserSubject = new BehaviorSubject<LoginResponse | null>(this.getCurrentUser());
  public currentUser$ = this.currentUserSubject.asObservable();

  // Constructor with Dependency Injection
  constructor(
    private http: HttpClient,
    private router: Router
  ) {
    console.log('AuthService initialized');
    // Check if token is expired on service initialization
    this.checkTokenExpiration();
  }

  // Login method
  login(credentials: LoginRequest): Observable<LoginResponse> {
    return this.http.post<LoginResponse>(`${this.apiUrl}/Users/login`, credentials,{ withCredentials: true })
      .pipe(
        tap(response => {
          this.setToken(response.token);
          this.setCurrentUser(response);
          this.isAuthenticatedSubject.next(true);
          console.log('Login successful, token saved');
        }),
        catchError(error => {
          console.error('Login error:', error);
          return throwError(() => error);
        })
      );
  }

  // Register method
  register(user: User): Observable<any> {
    return this.http.post(`${this.apiUrl}/Users/register`, user)
      .pipe(
        catchError(error => {
          console.error('Registration error:', error);
          return throwError(() => error);
        })
      );
  }

  // Logout method
  logout(): void {
    localStorage.removeItem(this.tokenKey);
    localStorage.removeItem(this.userKey);
    this.isAuthenticatedSubject.next(false);
    this.currentUserSubject.next(null);
    this.router.navigate(['/login']);
    console.log('User logged out');
  }

  // Token management
  getToken(): string | null {
    return localStorage.getItem(this.tokenKey);
  }

  private setToken(token: string): void {
    localStorage.setItem(this.tokenKey, token);
  }

  private hasToken(): boolean {
    return !!this.getToken();
  }

  // User management
  getCurrentUser(): LoginResponse | null {
    const user = localStorage.getItem(this.userKey);
    return user ? JSON.parse(user) : null;
  }

  private setCurrentUser(user: LoginResponse): void {
    localStorage.setItem(this.userKey, JSON.stringify(user));
    this.currentUserSubject.next(user);
  }

  // Check if user is logged in
  isLoggedIn(): boolean {
    return this.hasToken() && !this.isTokenExpired();
  }

  // Token expiration check
  private isTokenExpired(): boolean {
    const token = this.getToken();
    if (!token) return true;

    try {
      const payload = JSON.parse(atob(token.split('.')[1]));
      const expiry = payload.exp * 1000;
      return Date.now() >= expiry;
    } catch (e) {
      return true;
    }
  }

  private checkTokenExpiration(): void {
    if (this.hasToken() && this.isTokenExpired()) {
      this.logout();
    }
  }
}