import { Component, inject, input, OnInit, output } from '@angular/core';
import { Member } from '../../_models/member';
import { FileUploader, FileUploadModule } from 'ng2-file-upload';
import { DecimalPipe, NgClass, NgFor, NgIf, NgStyle } from '@angular/common';
import { AccountService } from '../../_services/account.service';
import { environment } from '../../../environments/environment';
import { Photo } from '../../_models/photo';
import { MembersService } from '../../_services/members.service';

@Component({
  selector: 'app-photo-editor',
  standalone: true,
  imports: [NgIf, NgClass, NgStyle, NgFor, FileUploadModule, DecimalPipe],
  templateUrl: './photo-editor.component.html',
  styleUrl: './photo-editor.component.scss',
})
export class PhotoEditorComponent implements OnInit {
  private accountService = inject(AccountService);
  private memberService = inject(MembersService);
  member = input.required<Member>();
  memberChanged = output<Member>();

  uploader?: FileUploader;
  hasBaseDropZoneOver = false;
  baseUrl = environment.apiUrl;

  ngOnInit(): void {
    this.initializeUploader();
  }

  setMainPhoto(photo: Photo) {
    this.memberService.setMainPhoto(photo).subscribe({
      next: () => {
        const user = this.accountService.currentUser();
        if (user) {
          user.photoUrl = photo.url;
          this.accountService.currentUser.set(user);
        }

        const updatedMember = { ...this.member() };
        updatedMember.photoUrl = photo.url;
        updatedMember.photos.forEach((p) => {
          if (p.isMain) p.isMain = false;
          if (p.id === photo.id) p.isMain = true;
        });
        this.memberChanged.emit(updatedMember);
      },
    });
  }

  deletePhoto(photo: Photo) {
    this.memberService.deletePhoto(photo).subscribe({
      next: () => {
        const updatedMember = { ...this.member() };
        updatedMember.photos = updatedMember.photos.filter(
          (p) => p.id !== photo.id
        );
        this.memberChanged.emit(updatedMember);
      },
    });
  }

  fileOverBase(e: any) {
    this.hasBaseDropZoneOver = e;
  }

  initializeUploader() {
    this.uploader = new FileUploader({
      url: this.baseUrl + 'users/add-photo',
      authToken: `Bearer ${this.accountService.currentUser()?.token}`,
      isHTML5: true,
      allowedFileType: ['image'],
      removeAfterUpload: true,
      autoUpload: false,
      maxFileSize: 10 * 1024 * 1024,
    });

    this.uploader.onAfterAddingFile = (file) => {
      file.withCredentials = false;
    };

    this.uploader.onSuccessItem = (item, response, status, headers) => {
      const photo = JSON.parse(response);
      const updatedMember = { ...this.member() };
      updatedMember.photos.push(photo);

      this.memberChanged.emit(updatedMember);

      if (photo.isMain) {
        const user = this.accountService.currentUser();
        if (user) {
          user.photoUrl = photo.url;
          this.accountService.currentUser.set(user);
        }
        updatedMember.photoUrl = photo.url;
        updatedMember.photos.forEach((p) => {
          if (p.isMain) p.isMain = false;
          if (p.id === photo.id) p.isMain = true;
        });
        this.memberChanged.emit(updatedMember);
      }
    };
  }
}
