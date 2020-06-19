import { Injectable } from '@angular/core';
import * as alertify from 'alertifyjs';

@Injectable({
  providedIn: 'root'
})
export class AlertifyService {

constructor() { }

// creating a function within the confirm so the user knows what to do if they click ok

confirm(message: string, okCallBack: () => any) {
  alertify.confirm(message, (e: any) => {
    if (e) {
      okCallBack();
    } else {

    }
  });
}

success(message: string){
  alertify.success(message);
}

error(message: string){
  alertify.error(message);
}

warning(message: string){
  alertify.warning(message);
}

message(message: string){
  alertify.message(message);
}

}
