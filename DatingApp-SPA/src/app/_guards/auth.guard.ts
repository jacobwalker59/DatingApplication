import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, UrlTree, Router } from '@angular/router';
import { Observable } from 'rxjs';
import { AuthService } from '../_services/Auth.service';
import { AlertifyService } from '../_services/alertify.service';

@Injectable({
  providedIn: 'root'
})

export class AuthGuard implements CanActivate {

  constructor(private authService: AuthService, private router: Router, private alertify: AlertifyService) {
  }
// if or else in here activates AuthGuard which activates the can activate method below
  canActivate():
     boolean {
       if (this.authService.loggedIn()) {
        return true;
       }
       this.alertify.error('You Shall Not Pass!');
       this.router.navigate(['/home']);
  }
}
