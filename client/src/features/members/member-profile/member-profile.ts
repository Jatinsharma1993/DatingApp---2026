import { Component, HostListener, inject, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { EditableMember, Member } from '../../../types/member';
import { DatePipe } from '@angular/common';
import { MemberService } from '../../../core/services/member-service';
import { FormsModule, NgForm } from '@angular/forms';
import { ToastService } from '../../../core/services/toast-service';
import { AccountService } from '../../../core/services/account-service';

@Component({
  selector: 'app-member-profile',
  imports: [DatePipe,FormsModule],
  templateUrl: './member-profile.html',
  styleUrl: './member-profile.css',
})
export class MemberProfile  implements OnInit, OnDestroy{
  
  @ViewChild('editForm') editForm? : NgForm;
  @HostListener('window:beforeunload',['$event']) notify($event : BeforeUnloadEvent){
    if(this.editForm?.dirty){
      $event.preventDefault();
    }
  }
  protected memberService = inject(MemberService);
  private toast = inject(ToastService)
  private accountService = inject(AccountService);
  protected editableMember : EditableMember ={
    userName : '',
    description : '',
    city :'',
    country : ''
  };

  ngOnInit(): void {
    this.editableMember = {
      userName : this.memberService.member()?.userName || '',
      description : this.memberService.member()?.description || '',
      city : this.memberService.member()?.city || '',
      country : this.memberService.member()?.country || '',
    }
  }
 
  updateProfile(){
    if(!this.memberService.member()) return;
    const updatedMember = {...this.memberService.member(),...this.editableMember};
    this.memberService.updateMember(this.editableMember).subscribe({
      next : () =>{
        let currentUser = this.accountService.currentUser();
        if(currentUser && currentUser.username !== updatedMember.userName){
          currentUser.username = updatedMember.userName;
          this.accountService.setCurrentUser(currentUser);
        }
        this.toast.success("Profile Updated successfully");
         this.memberService.editMode.set(false);
         this.memberService.member.set(updatedMember as Member);
         this.editForm?.reset(updatedMember);
      }
    })
  
  }

  ngOnDestroy(): void {
    if(this.memberService.editMode()){
      this.memberService.editMode.set(false);
    }
  }
}
