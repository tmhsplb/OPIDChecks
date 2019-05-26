using Microsoft.AspNet.Identity.EntityFramework;
using OPIDChecks.DataContexts;
using OPIDChecks.Models;
using OPIDEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OPIDChecks.DAL
{
    public class DataManager
    {
        private static RoleViewModel RoleEntityToRoleViewModel(IdentityRole idRole)
        {
            return new RoleViewModel
            {
                Id = idRole.Id,
                Name = idRole.Name
            };
        }

        public static List<RoleViewModel> GetRoles()
        {
            using (IdentityDB identitycontext = new DataContexts.IdentityDB())
            {
                List<RoleViewModel> roles = new List<RoleViewModel>();
                List<IdentityRole> identityRoles = identitycontext.Roles.ToList();

                foreach (IdentityRole idRole in identityRoles)
                {
                    roles.Add(RoleEntityToRoleViewModel(idRole));
                }

                return roles;
            }
        }

        private static InvitationViewModel InviteToInvitationViewModel(Invitation invite)
        {
            return new InvitationViewModel
            {
                Id = invite.Id,
                Extended = invite.Extended.ToString("MM/dd/yyyy"),
                Accepted = (invite.Accepted == (System.DateTime)System.Data.SqlTypes.SqlDateTime.Null ? string.Empty : invite.Accepted.ToString("MM/dd/yyyy")),
                UserName = invite.UserName,
                FullName = invite.FullName,
                Email = invite.Email,
                Role = invite.Role,
            };
        }
        public static List<InvitationViewModel> GetUsers()
        {
            using (OpidDB opidcontext = new DataContexts.OpidDB())
            {
                List<InvitationViewModel> invitations = new List<InvitationViewModel>();
                List<Invitation> invites = opidcontext.Invitations.ToList();

                foreach (Invitation invite in invites)
                {
                    invitations.Add(InviteToInvitationViewModel(invite));
                }

                return invitations;
            }
        }
    }
}