import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { Router } from '@angular/router';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private tokenKey = 'jwt_token';

  constructor(private http: HttpClient, private router: Router) {}

  register(user: any) {
    return this.http.post(`${environment.apiUrl}/auth/register`, user);
  }

  login(email: string, password: string) {
    return this.http.post<{ token: string }>(`${environment.apiUrl}/auth/login`, { email, password });
  }

  setToken(token: string) { localStorage.setItem(this.tokenKey, token); }
  getToken(): string | null { return localStorage.getItem(this.tokenKey); }
  isLoggedIn(): boolean { return !!this.getToken(); }

  isAdmin(): boolean {
    const token = this.getToken();
    if (!token) return false;
    try {
      const payload = JSON.parse(atob(token.split('.')[1]));
      const roles = payload.role || payload.roles || payload['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'];
      if (!roles) return false;
      if (Array.isArray(roles)) return roles.includes('Admin');
      return String(roles).split(',').map((r: string) => r.trim()).includes('Admin');
    } catch {
      return false;
    }
  }

  logout() {
    localStorage.removeItem(this.tokenKey);
    this.router.navigate(['/login']);
  }

  getUserId(): number | null {
    const token = this.getToken();
    if (!token) return null;
    try { const payload = JSON.parse(atob(token.split('.')[1])); return Number(payload.sub || payload.nameid || payload.userId); } catch { return null; }
  }
}
