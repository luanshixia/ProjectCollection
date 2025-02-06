//
//  AnalogClockView.swift
//  ClockLearner
//
//  Created by Yang Wang on 2/5/25.
//

import SwiftUI

struct ClockHand: View {
    var angle: Angle
    var length: CGFloat
    var color: Color
    
    var body: some View {
        Rectangle()
            .fill(color)
            .frame(width: 4, height: length)
            .offset(y: -length / 2)
            .rotationEffect(angle)
    }
}

struct AnalogClockView: View {
    @ObservedObject var viewModel: ClockViewModel
    
    var body: some View {
        ZStack {
            Circle()
                .stroke(Color.black, lineWidth: 2)
            ForEach(0..<12) { tick in
                Text("\(tick == 0 ? 12 : tick)")
                    .position(circlePosition(tick: tick))
            }
            
            ClockHand(angle: hourAngle(), length: 50, color: .black)
            ClockHand(angle: minuteAngle(), length: 90, color: .blue)
        }
    }
    
    func hourAngle() -> Angle {
        return Angle.degrees(Double(viewModel.hour % 12) * 30 + Double(viewModel.minute) * 0.5)
    }
    
    func minuteAngle() -> Angle {
        return Angle.degrees(Double(viewModel.minute) * 6)
    }
    
    func circlePosition(tick: Int) -> CGPoint {
        let angle = Angle.degrees(Double(tick) * 30 - 90)
        return CGPoint(x: 150 + cos(angle.radians) * 120, y: 150 + sin(angle.radians) * 120)
    }
}
