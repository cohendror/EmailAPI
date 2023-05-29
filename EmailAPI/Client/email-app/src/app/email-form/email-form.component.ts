import { Component } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-email-form',
  templateUrl: './email-form.component.html',
  styleUrls: ['./email-form.component.css']
})
export class EmailFormComponent {
  email: string = '';
  isValidEmail: boolean = false;
  responseTime: Date | null = null;

  constructor(private http: HttpClient) { }

  validateEmail() {
    //  email validation 
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    this.isValidEmail = emailRegex.test(this.email);
  }

  sendEmail() {
    this.http.post<any>('https://localhost:7278/api/Email', { email: this.email })
      .subscribe(response => {
        this.responseTime = new Date(response.serverTime);
      });
  }
}
