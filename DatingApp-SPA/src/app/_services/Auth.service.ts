import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs/operators';
import { JwtHelperService } from '@auth0/angular-jwt';
import { User } from '../_models/user';
import { BehaviorSubject } from 'rxjs';


@Injectable({
  providedIn: 'root'
})
// root module is app.module .ts
// weve been able to inject components, this is an injectable decorator
export class AuthService {

baseUrl = 'http://localhost:5000/auth/';

jwtHelper = new JwtHelperService();
currentUser: User;
decodedToken: any;
photoUrl = new BehaviorSubject<string>('../../assets/user.png');
currentPhotoUrl = this.photoUrl.asObservable();

constructor(private http: HttpClient ) { }
// in order to do anything with token we need to use SJX operators we need to put them in a pipe
// javascript functionality but with observables

changeMemberPhoto(photoUrl: string){
this.photoUrl.next(photoUrl);
}

login(model: any) {
  return this.http.post(this.baseUrl + 'login', model)
  .pipe(
    map((response: any) => {
    const user = response;
    if (user) { localStorage.setItem('token', user.token);
                localStorage.setItem('user', JSON.stringify(user.user));
                this.currentUser = user.user;
                this.decodedToken = this.jwtHelper.decodeToken(user.token);
                this.changeMemberPhoto(this.currentUser.mainPhoto);
               
    }
    
  }));

  // make sure you learn about how observables work

}

register(model: User) {
  return this.http.post(this.baseUrl + 'register', model);
}

loggedIn()
{
  const token = localStorage.getItem('token');
  return !this.jwtHelper.isTokenExpired(token);
}

}
