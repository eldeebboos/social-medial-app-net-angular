import { Component, inject, OnInit, TemplateRef } from '@angular/core';
import { AdminService } from '../../_services/admin.service';
import { User } from '../../_models/User';
import { BsModalService, BsModalRef, ModalOptions } from 'ngx-bootstrap/modal';
import { RolesModalComponent } from '../../_modals/roles-modal/roles-modal.component';

@Component({
  selector: 'app-user-management',
  standalone: true,
  imports: [],
  templateUrl: './user-management.component.html',
  styleUrl: './user-management.component.scss',
})
export class UserManagementComponent implements OnInit {
  private adminService = inject(AdminService);
  users: User[] = [];
  bsModalRef?: BsModalRef<RolesModalComponent> =
    new BsModalRef<RolesModalComponent>();

  private modalService = inject(BsModalService);

  ngOnInit(): void {
    this.getUsersWithRoles();
  }

  openRolesModal(user: User) {
    const initialState: ModalOptions = {
      class: 'modal-lg',
      initialState: {
        title: 'User roles',
        availableRoles: ['Admin', 'Moderator', 'Member'],
        username: user.username,
        selectedRoles: [...user.roles],
        users: this.users,
        rolesUpdate: false,
      },
    };
    this.bsModalRef = this.modalService.show(RolesModalComponent, initialState);

    this.bsModalRef.onHide?.subscribe(() => {
      if (this.bsModalRef?.content && this.bsModalRef?.content.rolesUpdated) {
        const selectedRoles = this.bsModalRef?.content.selectedRoles;
        this.adminService
          .updateUserRoles(user.username, selectedRoles)
          .subscribe((roles) => (user.roles = roles as string[]));
      }
    });
  }

  getUsersWithRoles() {
    this.adminService.getUsersWithRoles().subscribe((users) => {
      this.users = users;
    });
  }
}
