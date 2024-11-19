import { JsonPipe } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Component, inject, OnInit } from '@angular/core';
import { RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, JsonPipe],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss',
})
export class AppComponent implements OnInit {
  title = 'client';
  users: any;

  http = inject(HttpClient);

  ngOnInit(): void {
    this.http.get('http://localhost:5000/api/users').subscribe({
      next: (response) => {
        this.users = response;
      },
      error: (error) => {
        console.error(error);
      },
      complete: () => {
        console.log('Completed');
      },
    });
  }
}
