import { Component, computed, inject, input } from '@angular/core';
import { Member } from '../../_models/member';
import { RouterLink } from '@angular/router';
import { LikesService } from '../../_services/likes.service';
import { PresenceService } from '../../_services/presence.service';

@Component({
  selector: 'app-member-card',
  standalone: true,
  imports: [RouterLink],
  templateUrl: './member-card.component.html',
  styleUrl: './member-card.component.scss',
})
export class MemberCardComponent {
  private likeService = inject(LikesService);
  private presenceService = inject(PresenceService);
  member = input.required<Member>();
  hasLiked = computed(() => {
    return this.likeService.likeIds().includes(this.member().id);
  });
  isOnline = computed(() => {
    return this.presenceService.onlineUsers().includes(this.member().userName);
  });

  toggleLike() {
    this.likeService.toogleLike(this.member().id).subscribe({
      next: (_) => {
        if (this.hasLiked()) {
          this.likeService.likeIds.update((ids) =>
            ids.filter((x) => x !== this.member().id)
          );
        } else {
          this.likeService.likeIds.update((ids) => [...ids, this.member().id]);
        }
      },
    });
  }
}
