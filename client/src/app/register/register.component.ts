import { Component, inject, OnInit, output } from '@angular/core';
import {
  AbstractControl,
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  ValidatorFn,
  Validators,
} from '@angular/forms';
import { AccountService } from '../_services/account.service';
import { TextInputComponent } from '../_forms/text-input/text-input.component';
import { DatePickerComponent } from '../_forms/date-picker/date-picker.component';
import { Router } from '@angular/router';
import { catchError, tap } from 'rxjs';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [ReactiveFormsModule, TextInputComponent, DatePickerComponent],
  templateUrl: './register.component.html',
  styleUrl: './register.component.scss',
})
export class RegisterComponent implements OnInit {
  cancelRegister = output<boolean>();
  accountService = inject(AccountService);
  private router = inject(Router);
  validateionErrors: any; //string[] | undefined;

  fb = inject(FormBuilder);
  registerForm!: FormGroup;

  maxDate = new Date();

  ngOnInit(): void {
    this.initializeForm();
    this.maxDate.setFullYear(this.maxDate.getFullYear() - 18);
  }

  initializeForm() {
    this.registerForm = this.fb.group({
      username: ['', Validators.required],
      password: [
        '',
        [Validators.required, Validators.maxLength(6), Validators.minLength(3)],
      ],
      confirmPassword: [
        '',
        [Validators.required, this.matchValues('password')],
      ],
      gender: ['male'],
      knownAs: ['', Validators.required],
      dateOfBirth: ['', Validators.required],
      city: ['', Validators.required],
      country: ['', Validators.required],
    });

    //call the validation once we change the value of the password field
    this.registerForm.controls['password'].valueChanges.subscribe({
      next: () =>
        this.registerForm.controls['confirmPassword'].updateValueAndValidity(),
    });
  }

  register() {
    const dob = this.getDateOnly(this.registerForm.get('dateOfBirth')?.value);
    this.registerForm.patchValue({ dateOfBirth: dob });

    this.accountService.register(this.registerForm.value).subscribe({
      next: (_) => this.router.navigateByUrl('/members'),
      error: (errors) => (this.validateionErrors = errors),
    });
  }

  matchValues(matchTo: string): ValidatorFn {
    return (control: AbstractControl) => {
      return control.value === control.parent?.get(matchTo)?.value
        ? null
        : { notMatch: true };
    };
  }

  cancel() {
    this.cancelRegister.emit(false);
  }

  private getDateOnly(dob: string | undefined) {
    if (!dob) return;
    return new Date(dob).toISOString().slice(0, 10);
  }
}
