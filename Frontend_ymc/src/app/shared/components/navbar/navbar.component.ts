import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import { AuthService } from '../../../core/services/auth.service';
import { LoginResponse } from '../../../core/models/login.model';
import { Router } from '@angular/router';

@Component({
  selector: 'app-navbar',
  standalone: true,
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.css'],
 
})
export class NavbarComponent implements OnInit {
  isAuthenticated$!: Observable<boolean>;
  currentUser$!: Observable<LoginResponse | null>;

  constructor(
    private authService: AuthService,
    private router: Router
  ){
    console.log('NavbarComponent constructor called');
  }

  ngOnInit(): void {
    this.isAuthenticated$ = this.authService.isAuthenticated$;
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

  logout(): void {
    this.authService.logout();
  }
}
