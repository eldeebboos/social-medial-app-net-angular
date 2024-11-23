import { Component, inject, input, output } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { AccountService } from '../_services/account.service';
import { ToastrService } from 'ngx-toastr';

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
  private toaster = inject(ToastrService);

  register() {
    this.accountService.register(this.model).subscribe({
      next: (response) => {
        this.toaster.success('Registration successful!');

        this.cancel();
      },
      error: (error) => {
        console.error(error);
        this.toaster.error(error.error);
      },
    });
  }

  cancel() {
    this.cancelRegister.emit(false);
  }
}
