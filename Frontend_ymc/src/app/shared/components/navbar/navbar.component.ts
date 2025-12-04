import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import { AuthService } from '../../../core/services/auth.service';
import { LoginResponse } from '../../../core/models/auth.model';
import { Router, RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import Swal from 'sweetalert2';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.css'],
 
})
export class NavbarComponent implements OnInit {
  isAuthenticated$!: Observable<boolean>;
  currentUser$!: Observable<LoginResponse | null>;
  isLoggedIn = false;
  username: string = '';

  constructor(
    private authService: AuthService,
    private router: Router
  ){
    this.authService.isAuthenticated$.subscribe(
      isAuth => this.isLoggedIn = isAuth);
    console.log('NavbarComponent constructor called');
  }

  ngOnInit(): void {
    this.authService.isAuthenticated$.subscribe(
      (isAuth) => {
        this.isLoggedIn = isAuth;
        if (isAuth) {
          const user = this.authService.getCurrentUser();
          this.username = user?.unique_name || user?.username || user?.email || 'User';
        } else {
          this.username = '';
        }
      }
    );
    this.currentUser$ = this.authService.currentUser$;

    //navbar toggler
    const toggler = document.querySelector('.navbar-toggler');
    const navbarCollapse = document.querySelector('.navbar-collapse');
    
    toggler?.addEventListener('click', () => {
      navbarCollapse?.classList.toggle('show');
    });

     // จัดการ dropdown
    const dropdownToggle = document.querySelector('.dropdown-toggle');
    const dropdownMenu = document.querySelector('.dropdown-menu');
    
    dropdownToggle?.addEventListener('click', (e) => {
      e.preventDefault();
      dropdownMenu?.classList.toggle('show');
    });

    // ปิด menu เมื่อคลิกข้างนอก
    document.addEventListener('click', (e) => {
      const target = e.target as HTMLElement;
      if (!target.closest('.navbar')) {
        navbarCollapse?.classList.remove('show');
        dropdownMenu?.classList.remove('show');
      }
    });

  }

  onLogout(): void {
    if (confirm('Are you sure you want to logout?')) {
      this.authService.logout();
    }
  }

   // LogOut With API
  async onLogoutWithApi(): Promise<void> {
    if (confirm('Are you sure you want to logout?')) {
      await this.authService.logoutComplete();
    }
  }

  onLogoutWithConfirmation(): void {
    Swal.fire({
      title: 'Logout',
      text: 'Are you sure you want to logout?',
      icon: 'question',
      showCancelButton: true,
      confirmButtonColor: '#dc3545',
      cancelButtonColor: '#6c757d',
      confirmButtonText: 'Yes, logout',
      cancelButtonText: 'Cancel'
    }).then((result) => {
      if (result.isConfirmed) {
        this.authService.logout();
        
        Swal.fire({
          title: 'Logged Out!',
          text: 'You have been logged out successfully.',
          icon: 'success',
          timer: 1500,
          showConfirmButton: false
        });
      }
    });
  }


}
