import { Component, inject, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Member } from '../../_models/member';
import { TabDirective, TabsetComponent, TabsModule } from 'ngx-bootstrap/tabs';
import { GalleryModule, GalleryItem, ImageItem } from 'ng-gallery';
import { TimeagoModule } from 'ngx-timeago';
import { DatePipe } from '@angular/common';
import { MemberMessagesComponent } from '../member-messages/member-messages.component';
import { Message } from '../../_models/message';
import { MessagesService } from '../../_services/messages.service';
import { PresenceService } from '../../_services/presence.service';
import { AccountService } from '../../_services/account.service';

@Component({
  selector: 'app-member-detail',
  standalone: true,
  imports: [
    TabsModule,
    GalleryModule,
    TimeagoModule,
    DatePipe,
    MemberMessagesComponent,
  ],
  templateUrl: './member-detail.component.html',
  styleUrl: './member-detail.component.scss',
})
export class MemberDetailComponent implements OnInit, OnDestroy {
  @ViewChild('memberTabs', { static: true }) memberTabs?: TabsetComponent;
  presenceService = inject(PresenceService);
  private messageService = inject(MessagesService);
  private accountService = inject(AccountService);
  private route = inject(ActivatedRoute);
  member: Member = {} as Member;
  images: GalleryItem[] = [];

  activeTab?: TabDirective;

  ngOnInit(): void {
    this.route.data.subscribe((data) => {
      this.member = data['member'];
      this.member &&
        this.member.photos.map((photo) => {
          this.images.push(
            new ImageItem({
              src: photo.url,
              thumb: photo.url,
            })
          );
        });
    });
    this.route.queryParams.subscribe((params) => {
      params['tab'] && this.selectTab(params['tab']);
    });
  }
  ngOnDestroy(): void {
    this.messageService.stopHubConnection();
  }

  onTabActivated(data: TabDirective) {
    this.activeTab = data;
    if (this.activeTab.heading === 'Messages' && this.member) {
      const user = this.accountService.currentUser();
      if (!user) return;
      this.messageService.createHubConnection(user, this.member.userName);
    } else {
      this.messageService.stopHubConnection();
    }
  }

  selectTab(heading: string) {
    if (this.memberTabs) {
      const messageTab = this.memberTabs.tabs.find(
        (x) => x.heading === heading
      );
      if (messageTab) messageTab.active = true;
    }
  }
}
