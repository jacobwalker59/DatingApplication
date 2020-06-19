import { Component, OnInit } from '@angular/core';
import { AuthService } from './_services/Auth.service';
import { JwtHelperService } from '@auth0/angular-jwt';
import { User } from './_models/user';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})

// need to get main component to save token to local storage so that we can access it when we refresh
export class AppComponent implements OnInit{
  title = 'DatingApp-SPA';
  jwtHelper = new JwtHelperService();

  constructor(private authService: AuthService) {
  }
  ngOnInit(): void {
    const token = localStorage.getItem('token');
    const user: User = JSON.parse(localStorage.getItem('user'));
    if (token) {
      this.authService.decodedToken = this.jwtHelper.decodeToken(token);

    }
    if(user){
      this.authService.currentUser = user;
      this.authService.changeMemberPhoto(user.mainPhoto);
    }
  }
}
