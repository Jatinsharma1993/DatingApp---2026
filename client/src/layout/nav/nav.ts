import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { AccountService } from '../../core/services/account-service';
import { Router, RouterLink, RouterLinkActive } from "@angular/router";
import { ToastService } from '../../core/services/toast-service';

@Component({
  selector: 'app-nav',
  imports: [FormsModule, RouterLink, RouterLinkActive],
  templateUrl: './nav.html',
  styleUrl: './nav.css',
})
export class Nav {
  protected accountService = inject(AccountService);
  protected toastService = inject(ToastService);
  private router = inject(Router);
  protected creds : any = {};
  protected loggedIn = signal(false)
  
  login(){
    this.accountService.login(this.creds).subscribe({
      next : () =>{
         this.router.navigateByUrl('/members');
          this.toastService.success('logged in succesfully')
         this.creds = {};
      },
      error : (error : any) => {
        debugger
        this.toastService.error(error.error)
      }
    })
  }

  logout(){
     this.router.navigateByUrl('/')
    this.accountService.logout();
    
  }

}
