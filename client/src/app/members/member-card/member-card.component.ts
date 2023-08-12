import { Component, Input, OnInit, ViewEncapsulation } from '@angular/core';
import { Member } from 'src/app/Models/Member';

@Component({
  selector: 'app-member-card',
  templateUrl: './member-card.component.html',
  styleUrls: ['./member-card.component.css'],
})
export class MemberCardComponent{

    @Input() member: Member | undefined;

    

}
