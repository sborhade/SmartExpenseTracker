import { Component, OnInit } from '@angular/core';
import { AuthService } from '../services/auth.service';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css']
})
export class DashboardComponent implements OnInit {

  userName: string = '';

  constructor(private authService: AuthService) {}

  ngOnInit(): void {
    // Get the logged-in user's name or any relevant info from the auth service
    //this.userName = this.authService.getUserName(); // Assuming you have a method in AuthService
  }

  // Optionally, add a logout method
  logout(): void {
    this.authService.logout();
  }
}
