﻿using Alidu.Core.Domain;
using Alidu.Core.Domain.Interfaces;
using Alidu.Core.MessageBus.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Alidu.Core.MessageBus
{
    public abstract class DynamicMessageHandler : IDynamicMessageHandler
    {
        private readonly IIntegrationMessageService _integrationMessageService;
        private readonly IUnitOfWork _unitOfWork;

        public DynamicMessageHandler(IIntegrationMessageService integrationMessageService, IUnitOfWork unitOfWork)
        {
            _integrationMessageService = integrationMessageService;
            _unitOfWork = unitOfWork;
        }
        public async Task Handle(string correlationId, dynamic @event)
        {
            await _unitOfWork.ExecutionTransactionStrategy(async (transactionId, cancellationToken) =>
            {
                await DynamicMessageHandle(correlationId, @event);
            },
            afterCommitTransction: async (transactionId, cancellationToken) =>
            {
                await _integrationMessageService.PublishEventsThroughMessageBusAsync(transactionId);
            });
        }

        public abstract Task DynamicMessageHandle(string correlationId, dynamic @event);
    }
}
