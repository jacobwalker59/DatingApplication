import { Component, OnInit, Input } from '@angular/core';
import { Message } from 'src/app/_models/message';
import { UserService } from 'src/app/_services/user.service';
import { AuthService } from 'src/app/_services/Auth.service';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { tap } from 'rxjs/operators';

@Component({
  selector: 'app-member-messages',
  templateUrl: './member-messages.component.html',
  styleUrls: ['./member-messages.component.css']
})
export class MemberMessagesComponent implements OnInit {

@Input() recipientId: number;
messages: Message[];
newMessage: any = {};


  constructor(private userService: UserService, private authService: AuthService,private alertify: AlertifyService) { }

  ngOnInit() {
    this.loadMessages();
  }

  loadMessages(){
    const currentUserId = this.authService.decodedToken.nameId;
    this.userService.getMessageThread(this.authService.decodedToken.nameid, this.recipientId).pipe(
      tap(message => {
        // tslint:disable-next-line: prefer-for-of
        /*
        for(let i = 0; i < this.messages.length; i++)
        {
          if(this.messages[i].isRead === false && this.messages[i].recipientId === currentUserId) {
            this.userService.markAsRead(currentUserId, this.messages[i].id);
          }
        }
        */
      })
    ).subscribe
    (messages => {

      this.messages = messages;
      console.log('messages');
      console.log(this.messages);
    },error => {
      this.alertify.error(error);
    });
  }

  sendMessage() {
    this.newMessage.recipientId = this.recipientId;
    this.userService.sendMessage(this.authService.decodedToken.nameid, this.newMessage).subscribe((message: Message) => {
      debugger;
      this.messages.unshift(message);
      this.newMessage.content = '';
    }, error => {
      this.alertify.error(error);
    });
  }

}
