import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, BehaviorSubject, throwError } from 'rxjs';
import { tap, catchError } from 'rxjs/operators';
import { Router } from '@angular/router';
import { environment } from '../../../environments/environment';
import { LoginRequest, LoginResponse } from '../models/auth.model';
import { User } from '../models/user.model';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private apiUrl = `${environment.apiUrl}`;
  private tokenKey = 'jwt_token';
  private userKey = 'current_user';
  private refreshToken = 'refreshToken';
  private tokenExpirationTimer: any;
  
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

 
  sendOTP(email: string): Observable<any> {
    console.log('Sending OTP to:', email); 
    return this.http.post(`${this.apiUrl}/Auth/send-otp`, { email });
  }
  private hasToken(): boolean {
    return !!this.getToken();
  }

  // Login method
  login(credentials: LoginRequest): Observable<LoginResponse> {
    return this.http.post<LoginResponse>(`${this.apiUrl}/Auth/login`, credentials,{ withCredentials: true })
      .pipe(
        tap(response => {
          this.setToken(response.token);
          this.setCurrentUser(response);
          this.isAuthenticatedSubject.next(true);
          
          this.setAutoLogout(response.token);
          console.log('Login successful, token saved');
        }),
        catchError(error => {
          console.error('Login error:', error);
          return throwError(() => error);
        })
      );
  }
  private setAutoLogout(token: string): void {
    try {
      // Decode JWT เพื่อดึง expiration time
      const payload = JSON.parse(atob(token.split('.')[1]));
      const expirationDate = new Date(payload.exp * 1000);
      const timeUntilExpiration = expirationDate.getTime() - new Date().getTime();

      // ตั้ง timer
      if (timeUntilExpiration > 0) {
        this.tokenExpirationTimer = setTimeout(() => {
          this.logout();
          alert('Your session has expired. Please login again.');
        }, timeUntilExpiration);
      }
    } catch (error) {
      console.error('Error setting auto logout:', error);
    }
  }

  // Logout method
  logout(): void {
    
    localStorage.removeItem(this.tokenKey);
    localStorage.removeItem(this.userKey);
    localStorage.removeItem(this.refreshToken);
    this.isAuthenticatedSubject.next(false);
    this.currentUserSubject.next(null);
    this.router.navigate(['/login']);
    console.log('User logged out');
  }
  logoutWithApi(): Observable<any> {
    return this.http.post(`${this.apiUrl}/Auth/logout`, {});
  }
  async logoutComplete(): Promise<void> {
    try {
      // เรียก API logout (ถ้ามี)
      await this.logoutWithApi().toPromise();
    } catch (error) {
      console.error('Logout API error:', error);
    } finally {
      // ลบ token และ redirect (ทำอยู่แล้วแม้ API error)
      this.logout();
    }
  }

  
  // Register method
  register(user: User): Observable<any> {
    return this.http.post(`${this.apiUrl}/Auth/register`, user)
      .pipe(
        catchError(error => {
          console.error('Registration error:', error);
          return throwError(() => error);
        })
      );
  }

  // Token management
  getToken(): string | null {
    return localStorage.getItem(this.tokenKey);
  }
   isLoggedIn(): boolean {
    return this.hasToken() && !this.isTokenExpired();
  }

  private setToken(token: string): void {
    localStorage.setItem(this.tokenKey, token);
  }

  // User management
  getCurrentUser(): any {
    const token = this.getToken();
    if (token) {
      try {
        const payload = JSON.parse(atob(token.split('.')[1]));
        return payload;
      } catch (e) {
        return null;
      }
    }
    return null;
  }
  // getCurrentUser(): LoginResponse | null {
  //   const user = localStorage.getItem(this.userKey);
  //   return user ? JSON.parse(user) : null;
  // }

  private setCurrentUser(user: LoginResponse): void {
    localStorage.setItem(this.userKey, JSON.stringify(user));
    this.currentUserSubject.next(user);
  }

  // Check if user is logged in
 

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