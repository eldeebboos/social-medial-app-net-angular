import { Component, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { AccountService } from '../_services/account.service';
import { BsDropdownModule } from 'ngx-bootstrap/dropdown';
import { Router, RouterLink, RouterLinkActive } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { HasRoleDirective } from '../_directivs/has-role.directive';

@Component({
  selector: 'app-nav',
  standalone: true,
  imports: [
    FormsModule,
    RouterLink,
    RouterLinkActive,
    BsDropdownModule,
    HasRoleDirective,
  ],
  templateUrl: './nav.component.html',
  styleUrl: './nav.component.scss',
})
export class NavComponent {
  accountService = inject(AccountService);
  router = inject(Router);
  private toaster = inject(ToastrService);

  model: any = {};

  login() {
    this.accountService.login(this.model).subscribe({
      next: (response) => {
        this.accountService.login(response);
        this.router.navigate(['members']);
        this.toaster.success('Login successful!');
      },
      error: (error) => {
        console.error(error);
        this.toaster.error(error.error);
      },
    });
  }
  logout() {
    this.accountService.logout();
    this.router.navigate(['/']);
  }
}
