import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from './services/auth.service';

export const authGuard: CanActivateFn = (route, state) => {

  const authService = inject(AuthService);
  const router = inject(Router);

  //Check if the user is authenticated
  if (authService.isAuthenticated()) {
    return true;
  }

  //Redirect to login page if the user is not authenticated
  router.navigate(['/login']);
  return false;

};
