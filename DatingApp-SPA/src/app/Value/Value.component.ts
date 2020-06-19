import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-Value',
  templateUrl: './Value.component.html',
  styleUrls: ['./Value.component.css']
})
export class ValueComponent implements OnInit {

  values: any;

  constructor(private http: HttpClient) 
  // dependency injection of client, app.module .ts is essenitally just the startup class.
  { }

  ngOnInit() {
  this.getValues();
  }

  getValues() {
    this.http.get('http://localhost:5000/values').subscribe(response => {
      
    this.values = response;
    // vaues which has a variable of anything like var is set to the response that we are getting above.
  }, error => {
    console.log(error);
  }
    );
    // returns an observable as a JSON object.
    // observable is like a newsletter that we subscribe to, but we have to subscribe to it in order
    // to get the information out of it.
  }
}
