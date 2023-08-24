import { ResolveFn } from '@angular/router';
import { Member } from '../Models/Member';
import { inject } from '@angular/core';
import { MembersService } from '../Services/members.service';

export const memberDetailedResolver: ResolveFn<Member> = (route, state) => {
    
    const memberService = inject(MembersService);
    return memberService.getMember(route.paramMap.get('username')!);
};
