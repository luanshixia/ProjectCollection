//
//  ContentView.swift
//  ClockLearner
//
//  Created by Yang Wang on 2/5/25.
//

import SwiftUI

struct ContentView: View {
    @StateObject private var viewModel = ClockViewModel()
        
    var body: some View {
        VStack {
            AnalogClockView(viewModel: viewModel)
                .frame(width: 300, height: 300)
                .contentShape(Rectangle())
                .gesture(DragGesture().onChanged { value in
                    updateTimeFromDrag(value: value, viewModel: viewModel)
                })
            
            DigitalClockView(viewModel: viewModel)
                .padding()
        }
    }
    
    func updateTimeFromDrag(value: DragGesture.Value, viewModel: ClockViewModel) {
        let center = CGPoint(x: 150, y: 150)
        let dragPoint = value.location
        let dx = dragPoint.x - center.x
        let dy = dragPoint.y - center.y
        let angle = atan2(dy, dx) * 180 / .pi + 90
        let fixedAngle = angle < 0 ? angle + 360 : angle
        
        let newMinute = Int(fixedAngle / 6) % 60
        
        if newMinute == 59 && viewModel.minute == 0 {
            viewModel.hour = (viewModel.hour - 1 + 12) % 12
            if viewModel.hour == 0 { viewModel.hour = 12 }
        } else if newMinute == 0 && viewModel.minute == 59 {
            viewModel.hour = (viewModel.hour + 1) % 12
            if viewModel.hour == 0 { viewModel.hour = 12 }
        }
        
        viewModel.minute = newMinute
    }
}

#Preview {
    ContentView()
}
