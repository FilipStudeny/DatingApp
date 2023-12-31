import { Component, Input, OnInit, ViewEncapsulation } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { Member } from 'src/app/Models/Member';
import { PresenceService } from 'src/app/Services/chat/presence.service';
import { MembersService } from 'src/app/Services/members.service';

@Component({
  selector: 'app-member-card',
  templateUrl: './member-card.component.html',
  styleUrls: ['./member-card.component.css'],
})
export class MemberCardComponent implements OnInit{

    @Input() member: Member | undefined;

    constructor(private memeberService: MembersService, private toastr: ToastrService, public presenceService: PresenceService) {}
    ngOnInit(): void {
    }


    addLike(member: Member){
        this.memeberService.addLike(member.username).subscribe({
            next: () => this.toastr.success('You have liked ' + member.knownAs)
        })
    }

}
