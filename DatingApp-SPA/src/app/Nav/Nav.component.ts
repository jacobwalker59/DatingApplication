import { Component, OnInit } from '@angular/core';
import { AuthService } from '../_services/Auth.service';
import { AlertifyService } from '../_services/alertify.service';
import { Route } from '@angular/compiler/src/core';
import { Router } from '@angular/router';

@Component({
  // tslint:disable-next-line: component-selector
  selector: 'app-Nav',
  templateUrl: './Nav.component.html',
  styleUrls: ['./Nav.component.css']
})
export class NavComponent implements OnInit {

  model: any = {};
  photoUrl: string;
  constructor(public authService: AuthService, private alertify: AlertifyService, private router: Router) {

  }

  ngOnInit() {
    this.authService.currentPhotoUrl.subscribe(photoUrl => this.photoUrl = photoUrl);
    console.log(this.authService.currentUser);
  }

  login() {
    this.authService.login(this.model).subscribe(next => {
      this.alertify.success('Logged In Successfully');
      
      
    }, error => {
      console.log('Failed To Login');
      this.alertify.error(error);
    }, () => {
      this.router.navigate(['/members']);
    });

    }

    loggedIn() {
      const token = localStorage.getItem('token');
      if (token) {
       
      } else {
       
      }

      return !!token;
      // short hand for if statement, if it is this return true, otherwise return false;
    }

    logout() {
      localStorage.removeItem('token');
      localStorage.removeItem('user');
      this.authService.decodedToken = null;
      this.authService.currentUser = null;
      this.alertify.message('Logged Out');
      this.router.navigate(['/home']);
    }

  }


