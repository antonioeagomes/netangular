using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Backend.DTO;
using Backend.Entities;
using Backend.Helpers;
using Backend.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Backend.Data
{
    public class MessageRepository : IMessageRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        public MessageRepository(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public void AddMessage(Message message)
        {
            _context.Messages.Add(message);
        }

        public void DeleteMessage(Message message)
        {
            _context.Messages.Remove(message);
        }

        public async Task<Message> GetMessage(int id)
        {
            return await _context.Messages
                        .Include(u => u.Sender).Include(u => u.Recipient)
                        .SingleOrDefaultAsync(m => m.Id == id);
        }

        public async Task<PagedList<MessageDTO>> GetMessagesForUser(MessageParams messageParams)
        {
            var query = _context.Messages
                .OrderByDescending(m => m.SentAt)
                .ProjectTo<MessageDTO>(_mapper.ConfigurationProvider)
                .AsQueryable();

            query = messageParams.Container switch 
            {
                "Inbox" => query.Where(u => u.RecipientUsername == messageParams.Username && !u.RecipientDeleted),
                "Outbox"=> query.Where(u => u.SenderUsername == messageParams.Username && !u.SenderDeleted),
                _ => query.Where(u => u.RecipientUsername == messageParams.Username && !u.RecipientDeleted && u.ReadAt == null)
            };

            return await PagedList<MessageDTO>.CreateAsync(query, messageParams.PageNumber,messageParams.PageSize);

        }

        public async Task<IEnumerable<MessageDTO>> GetMessageThread(string currentUsername, string recipientUsername)
        {
            var messages = await _context.Messages
                .Where(m => (m.Recipient.UserName == currentUsername && !m.RecipientDeleted
                    && m.Sender.UserName == recipientUsername)
                    || (m.Recipient.UserName == recipientUsername && !m.SenderDeleted
                    && m.Sender.UserName == currentUsername))
                .OrderByDescending(m => m.SentAt)
                .ProjectTo<MessageDTO>(_mapper.ConfigurationProvider)
                .ToListAsync();
            
            var unreadMessages = messages.Where(m => m.ReadAt == null 
                && m.RecipientUsername == currentUsername).ToList();

            if (unreadMessages.Any()) 
            {
                foreach (var message in unreadMessages)
                {
                    message.ReadAt = DateTime.UtcNow;
                }

                await _context.SaveChangesAsync();
            }

            return messages;
        }
    }
}