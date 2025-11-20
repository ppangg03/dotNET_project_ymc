import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import { AuthService } from '../../../core/services/auth.service';
import { LoginResponse } from '../../../core/models/login.model';

@Component({
  selector: 'app-navbar',
  standalone: true,
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.css'],
 
})
export class NavbarComponent implements OnInit {
  isAuthenticated$!: Observable<boolean>;
  currentUser$!: Observable<LoginResponse | null>;

  constructor(private authService: AuthService) {
    console.log('NavbarComponent constructor called');
  }

  ngOnInit(): void {
    this.isAuthenticated$ = this.authService.isAuthenticated$;
    this.currentUser$ = this.authService.currentUser$;
  }

  logout(): void {
    this.authService.logout();
  }
}
