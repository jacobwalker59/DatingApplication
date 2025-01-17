import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { AuthService } from '../_services/Auth.service';
import { AlertifyService } from '../_services/alertify.service';
import { FormGroup, FormControl, Validators, FormBuilder } from '@angular/forms';
import { BsDatepickerConfig } from 'ngx-bootstrap/datepicker/public_api';
import { User } from '../_models/user';
import { Router } from '@angular/router';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
  @Input() valuesFromHome: any;
  @Output() cancelRegister = new EventEmitter();
  user: User;
  registerForm: FormGroup;
  bsConfig: Partial<BsDatepickerConfig>;


  constructor(private authService: AuthService,
              private alertify: AlertifyService, private fb: FormBuilder, private router:Router) { }

  ngOnInit() {
    this.bsConfig = {
      containerClass: 'theme-green'
    };
    console.log(this.valuesFromHome);
    this.createRegisterForm();
  }

  createRegisterForm() {
    this.registerForm = this.fb.group({
      gender: ['male'],
      username: ['', Validators.required],
      knownAs: ['', Validators.required],
      dateOfBirth: [null, Validators.required],
      city: ['', Validators.required],
      country: ['', Validators.required],
      password: ['', [Validators.required, Validators.minLength(4), Validators.maxLength(8)]],
      confirmPassword: ['', Validators.required]
    },{ validator: this.passwordMatchValidator});
  }

  passwordMatchValidator(g: FormGroup){
    // tslint:disable-next-line: object-literal-key-quotes
    return g.get('password').value === g.get('confirmPassword').value ? null : {'mismatch': true};
  }

  register() {
    // this.authService.register(this.model).subscribe(() =>
    // {
    //   this.alertify.message('Registration Successful');
    // }, error => {
    //   console.log(error);
    //   console.log(this.model);
    //   console.log(this.model.Password);
    //   console.log(this.model.Username);
    // });
    if(this.registerForm.valid){
      this.user = Object.assign({}, this.registerForm.value);
      this.authService.register(this.user).subscribe(() => this.alertify.success('Registration Successful'),
      error => {
        this.alertify.error(error);
      }, () => { this.authService.login(this.user).subscribe(()=> {
        this.router.navigate(['members']);
      }); });
    }
  }

  cancel() {
    this.cancelRegister.emit(false);
    console.log('cancelled');
    this.alertify.message('Cancelled');
  }

}
