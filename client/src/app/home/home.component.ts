import { Component, inject, OnInit } from '@angular/core';
import { RegisterComponent } from '../register/register.component';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [RegisterComponent],
  templateUrl: './home.component.html',
  styleUrl: './home.component.scss',
})
export class HomeComponent implements OnInit {
  registerMode = false;
  users: any;
  http = inject(HttpClient);

  ngOnInit() {
    this.getUsers();
  }

  RegisterToggle() {
    this.registerMode = !this.registerMode;
  }

  getUsers() {
    this.http.get('http://localhost:5000/api/users').subscribe({
      next: (response) => {
        this.users = response;
      },
      error: (error) => {
        console.error(error);
      },
      complete: () => {
        // console.log('Completed');
      },
    });
  }

  cancelRegisterMode(event: boolean) {
    this.registerMode = false;
  }
}
