import { Component } from '@angular/core';
import { RegisterService } from '../services/register.service'; // Import the RegisterService
import { Router } from '@angular/router';
import { catchError } from 'rxjs/operators';
import { of } from 'rxjs';


@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})

export class RegisterComponent {
  username: string = '';
  password: string = '';
  errorMessage: string = '';
  successMessage: string = '';

  constructor(private registerService: RegisterService, private router: Router) { }

  // Method to handle registration
  onRegister(): void {
    this.registerService.register(this.username, this.password).pipe(
      catchError((error) => {
        this.errorMessage = error.error?.message || 'An error occurred during registration';
        return of(null); // Return null in case of error to prevent crash
      })
    ).subscribe({
      next: (response: any) => {
        if (response) {
          this.successMessage = 'Registration successful! You can now login.';
          // Optionally redirect the user to login page after a timeout
          setTimeout(() => this.router.navigate(['/login']), 2000);
        }
      },
      error: () => {
        this.errorMessage = 'There was an error while registering. Please try again later.';
      }
    });
  }
}
