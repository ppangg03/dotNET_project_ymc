import { Component, OnInit } from '@angular/core';
import { Router, NavigationEnd, RouterOutlet } from '@angular/router';
import { filter } from 'rxjs/operators';
import { CommonModule } from '@angular/common';
import { NavbarComponent } from './app/shared/components/navbar/navbar.component';
import { FooterComponent } from './app/shared/components/footer/footer.component';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [ 
    RouterOutlet, 
    CommonModule,
    NavbarComponent,
    FooterComponent
  ],
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css'],
  
  
})
export class AppComponent implements OnInit {
  title = 'Angular Auth App';

  constructor(private router: Router) {
    console.log('AppComponent constructor called - Root component initialized');
  }

  ngOnInit(): void {
    // Track route changes
    this.router.events.pipe(
      filter(event => event instanceof NavigationEnd)
    ).subscribe((event: any) => {
      console.log('Navigated to:', event.url);
    });
  }
}