import { Component, inject, input, output } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { AccountService } from '../_services/account.service';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './register.component.html',
  styleUrl: './register.component.scss',
})
export class RegisterComponent {
  model: any = {};

  cancelRegister = output<boolean>();
  accountService = inject(AccountService);

  register() {
    this.accountService.register(this.model).subscribe({
      next: (response) => {
        console.log('Registration successful!', response);

        this.cancel();
      },
      error: (error) => {
        console.error(error);
      },
    });
  }

  cancel() {
    this.cancelRegister.emit(false);
  }
}
