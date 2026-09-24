import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { AuthService } from '../../services/auth.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html'
})
export class LoginComponent {
  form!: FormGroup;
  error = '';

  constructor(private fb: FormBuilder, private auth: AuthService, private router: Router) {}

  ngOnInit(): void {
    this.form = this.fb.group({
      email: ['', [Validators.required]],
      password: ['', [Validators.required]]
    });
  }

  submit(){
    if (this.form.invalid) return;
    const v = this.form.value;
    this.auth.login(v.email!, v.password!).subscribe({
      next: r => {
        const token = (r as any)?.token;
        if (token) {
          this.auth.setToken(token);
          this.router.navigate(['/']);
        } else {
          this.error = 'Login did not return a token';
          console.error('Login response', r);
        }
      },
      error: (e) => {
        console.error('Login error', e);
        // Try to show useful message from backend if available
        this.error = e?.error?.error || e?.error?.message || `Login failed (${e?.status || 'unknown'})`;
      }
    });
  }
}
