import {  Routes } from '@angular/router';
import { LoginComponent } from './app/features/auth/login/login.component';
import { DashboardComponent } from './app/features/dashboard/dashboard.component';
import { AuthGuard } from './app/core/guards/auth.guard';


export const routes: Routes = [
  { path: '', redirectTo: 'dashboard', pathMatch: 'full' },

  { path: 'login', component: LoginComponent },

  { path: 'dashboard', component: DashboardComponent, canActivate: [AuthGuard] },

  { path: '**', redirectTo: 'dashboard' }
];