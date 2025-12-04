import {  Routes } from '@angular/router';
import { LoginComponent } from './app/features/auth/login/login.component';
import { DashboardComponent } from './app/features/dashboard/dashboard.component';
import { AuthGuard } from './app/core/guards/auth.guard';
import { HomeComponent } from './app/features/home/home.component';
import { RegisterComponent } from './app/features/auth/register/register.component';


export const routes: Routes = [
  { path: '', component: HomeComponent },
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent},
  { path: 'dashboard', component: DashboardComponent, canActivate:[AuthGuard]},
  { path: 'home', component: HomeComponent},
  { path: '**', redirectTo: '' }


  // { path: '**', redirectTo: 'dashboard' }
];