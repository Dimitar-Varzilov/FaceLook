const participants = new Set();

function toggleParticipant(friendId, element) {
    const button = document.getElementById('createChatBtn');

    if (participants.has(friendId)) {
        participants.delete(friendId);
        element.querySelector('i').className = 'bi bi-circle';
    } else {
        participants.add(friendId);
        element.querySelector('i').className = 'bi bi-check-circle-fill text-success';
    }

    button.disabled = participants.size === 0;
}

function getParticipants() {
    return Array.from(participants);
}

function clearParticipantsUI() {
    participants.forEach(participantId => {
        const element = document.querySelector(`[data-friend-id="${participantId}"]`);
        if (element) {
            element.querySelector('i').className = 'bi bi-circle';
        }
    });
    participants.clear();
    document.getElementById('createChatBtn').disabled = true;
}
